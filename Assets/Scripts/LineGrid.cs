using UnityEngine;

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
public class LineGrid : MonoBehaviour
{
    Vector3[] verts;    //�|���S���̒��_������
    int[] triangles;    //�O�p�`��`���ۂɁA���_�̕`�揇���w�肷��
    GameObject camera;  //�J����

    [SerializeField, Header("�g�p����Material")] Material material;
    [SerializeField, Header("�傫��")] Vector2Int size;
    [SerializeField, Header("���̑���")] float lineSize;

    void Start()
    {
        //�J�������擾
        camera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        CreateGlid();

        //�J�������O���b�h�̒��S��(�K�v�Ȃ��ꍇ�̓R�����g�A�E�g���Ă�������)
        //camera.transform.position = new Vector3((float)size.x / 2, ((float)size.y / 2) - 0.1f, -10);
    }

    void CreateGlid()
    {
        //�V����Mesh���쐬
        Mesh mesh = new Mesh();

        //���_�̔ԍ���size���m�ہA�c���̐�����{���Ȃ��Ȃ�̂�+2������A��{�̐��͒��_6�ŕ\��������̂�*6
        triangles = new int[(size.x + size.y + 2) * 6];
        //���_�̍��W��size���m��
        verts = new Vector3[(size.x + size.y + 2) * 6];

        //���_�ԍ������蓖��
        for (int i = 0; i < triangles.Length; i++)
        {
            triangles[i] = i;
        }


        //����for��������������J�E���g������
        int x = 0, y = 0;

        //�c��
        for (int i = 0; i < (size.x + 1) * 6; i += 6)
        {
            verts[i] = new Vector3(x, 0, 0);
            verts[i + 1] = new Vector3(x, size.y, 0);
            verts[i + 2] = new Vector3(lineSize + x, size.y, 0);
            verts[i + 3] = new Vector3(lineSize + x, size.y, 0);
            verts[i + 4] = new Vector3(lineSize + x, 0, 0);
            verts[i + 5] = new Vector3(x, 0, 0);
            x++;
        }

        //����
        for (int i = (size.x + 1) * 6; i < (size.x + size.y + 2) * 6; i += 6)
        {
            verts[i] = new Vector3(0, y, 0);
            verts[i + 1] = new Vector3(size.x + lineSize, y, 0);
            verts[i + 2] = new Vector3(0, y - lineSize, 0);
            verts[i + 3] = new Vector3(size.x + lineSize, y, 0);
            verts[i + 4] = new Vector3(size.x + lineSize, y - lineSize, 0);
            verts[i + 5] = new Vector3(0, y - lineSize, 0);
            y++;
        }

        //��������_�ԍ��A���W�f�[�^���쐬����mesh�ɒǉ�
        mesh.vertices = verts;
        mesh.triangles = triangles;

        //�Čv�Z()
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        //�Čv�Z��Ɋ�������Mesh��ǉ�
        GetComponent<MeshFilter>().mesh = mesh;
        //�ݒ肵��Material�𔽉f
        GetComponent<MeshRenderer>().material = material;
    }
}

